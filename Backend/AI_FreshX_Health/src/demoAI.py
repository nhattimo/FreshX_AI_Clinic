from flask import Flask
from flask_sqlalchemy import SQLAlchemy

app = Flask(__name__)
app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///clinic.db'
db = SQLAlchemy(app)

class Disease(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(255), nullable=False)
    description = db.Column(db.Text, nullable=False)
    symptoms = db.Column(db.Text, nullable=False)
    treatment = db.Column(db.Text, nullable=True)

class Patient(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(255), nullable=False)
    age = db.Column(db.Integer, nullable=False)
    symptoms = db.Column(db.Text, nullable=False)
    diagnosis = db.Column(db.String(255), nullable=True)

db.create_all()

# Thêm dữ liệu mẫu
disease1 = Disease(name="Bệnh cúm", description="Bệnh do virus cúm gây ra", symptoms="Sốt, ho, đau họng", treatment="Nghỉ ngơi, uống nhiều nước")
disease2 = Disease(name="Tiểu đường", description="Bệnh do rối loạn chuyển hóa đường", symptoms="Khát nước, tiểu nhiều, mệt mỏi", treatment="Dùng thuốc, thay đổi chế độ ăn uống")

db.session.add(disease1)
db.session.add(disease2)
db.session.commit()

@app.route('/get_disease', methods=['GET'])
def get_disease():
    disease_name = request.args.get('name')
    disease = Disease.query.filter_by(name=disease_name).first()
    if disease:
        return jsonify({
            'name': disease.name,
            'description': disease.description,
            'symptoms': disease.symptoms,
            'treatment': disease.treatment
        })
    else:
        return jsonify({'message': 'Disease not found'}), 404

if __name__ == '__main__':
    app.run(debug=True)
